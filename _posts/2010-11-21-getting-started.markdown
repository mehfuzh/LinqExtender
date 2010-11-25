---
layout : default
title : Gettting Started
---

### A simple Text provider using LinqExtender.

The goal of this post is to get started creating custom provider using LinqExtender.


To begin, let's say i want to build a simple provider. The context or entry-point class that will be queried upon, need to first implement the following interface:


	public interface IQueryContext<T>
	{
	  IEnumerable<T> Execute(Ast.Expression expression);  
	}

The interface has only one method named _Execute_ that accepts translated expression that is poplulated by the extender and which we will be visiting to produce TSQL statement.

Generally, the expresion will be traversed by visitor pattern, one can write the vistor class specific to LinqExtender then include as an base class and override its various methods to build the expected meta that will be run against a data store or send over HTTP to produce the expected result.

To smooth things up, a similar class is provided in LinqExtender.Tests project to start as a reference.

Before doing a deep dive. Let me do an short introduction on how the simiplied tree is constructed.

Let me consider the following LINQ query:

	var query = from book in context
				where book.Id  = 1
				select book

This is translated into :
	
	BlockExpression
		TypeExpression - Contains various reflected accessors on query object.
		LambdaExpression - Represents the where clause.
			BinaryExpression
				MemberExpression - Contains member related method and accessors
				LiteralEpxression - Contains the evaluted value.

Moving forward to a bit complex query:

	var query = from book in context
		where (book.Id > 1) && (book.Author == "Scott" || book.Author == "John")
		Select book

It is translated to:

	BlockExpression
		TypeExpression : Name == "Book", If NameAttribute applied then the specified one.
		LambdaExpression
			LogicalExpresion - Contains the logical parts | Operator = LogicalOperator.AND
				BinaryExpression 
					MemberExpression - 
						Name == "Id"
					LiteralExpression
						Value = 1	
				BinaryExpression
					LogicalExpression : Operator = LogicalOperator.OR
						BinaryExpression
							MemberExpression - 
								Name == "Author"
							LiteralExpression
								Value = "Scott"	
						BinaryExpression
							MemberExpression - 
								Name == "Author"
							LiteralExpression
								Value = "John"	
									
Here , BinaryExpresion or LambdaExpression is LinqExtender's version and thus all the expression contains various accesors and methods that easily let you get the query data.

Moving forward, Lets add orderby to our first query:

	var query = from book in context
				where book.Id  = 1
				orderby book.Author asc
				select book

This will be translated to:

	BlockExpression
		TypeExpression
		LambdaExpression
			BinaryExpression
				MemberExpression
				LiteralEpxression
		OrderByExpression

If you write the above query in the following way:

	var query = from book in context
				where book.Id  = 1
				orderby book.Author asc
				select new { book.Id, book.Author };
				
It will also produce the same tree as above. Therefore, it is projected and parsed internally.

In our sample Text provider, output will be stored in a StringBuilder and we can then print it out to your desired
media or compare it with our expected.

	var builder = new StringBuilder();
	var context = new TextContext<Book>(new StringWriter(builder));

	var query = from book in context
			    where book.Id == 10 
			    || (book.Id == 1 && book.Author == "Charlie")
			    select book;

	query.Count();

	Console.WriteLine(builder.ToString());				

The first step is to implment IQueryContext interface to the TextContext 

	public IEnumerable<T> Execute(Ast.Expression expression)
	{
		this.Visit(expression);
		return new List<T>().AsEnumerable();
	}

Since here result is not important, therefore returned a new instance of List. Addtionally, we have included the ExpressionVisitor from which we will be overriding methods to get our expected output.

Roughly the ExpresisonVisitor.Visit(Ast.Expresion) looks like:

	internal Ast.Expression Visit(Ast.Expression expression)
	{
		switch (expression.CodeType)
		{
		    case CodeType.BlockExpression:
		        return VisitBlockExpression((Ast.BlockExpression)expression);
		    case CodeType.TypeExpression:
		        return VisitTypeExpression((Ast.TypeExpression)expression);
		    case CodeType.LambdaExpresion:
		        return VisitLambdaExpression((Ast.LambdaExpression)expression);
		    case CodeType.LogicalExpression:
		        return VisitLogicalExpression((Ast.LogicalExpression)expression);
		    case CodeType.BinaryExpression:
		        return VisitBinaryExpression((Ast.BinaryExpression)expression);
		    case CodeType.LiteralExpression:
		        return VisitLiteralExpression((Ast.LiteralExpression)expression);
		    case CodeType.MemberExpression:
		        return VisitMemberExpression((Ast.MemberExpression)expression);
		    case CodeType.OrderbyExpression:
		        return VisitOrderbyExpression((Ast.OrderbyExpression)expression);
		}

		throw new ArgumentException("Expression type is not supported");
	}

If we follow the translated flow, the first expression that I am interested is the TypeExperession where i will be appending the Select * From {TypeName}, can the entry point the REST method names as well like flickr.photos.getList

	public override Ast.Expression VisitTypeExpression(Ast.TypeExpression expression)
	{
		writer.Write(string.Format("select * from {0}", expression.Type.Name));
		return expression;
	}
	
Now, expression.Type is not System.Type rather its LinqExtender.TypeReference, the Name returns either the original typename or the name that user specifies on top of the class through LinqExtender.NameAttribute , let's for example take the following class:

	[Name("flickr.photos.search")]
	public class Photo
	{

	}

Now if we take this part:
where book.Id == 10 || (book.Id == 1 && book.Author == "Charlie")

It will be translated like this

BinaryExpression
LogicalExpression
	BinaryExpression
	BinaryExpression

To print / generate the equivalant TSQL for it , we first of all not need to worry about the order in which the gropings are made or the level of nested groupings used in the query. While visiting the expression , our task is to prepare the meta for the respected expression only and during the execution it will be inovoked by extender as it is specified in the query.
	
Therefore, inside VisitLogicalExpression, I wrote :

	public override Ast.Expression VisitLogicalExpression(Ast.LogicalExpression expression)
	{
		WriteTokenIfReq(expression, Token.LeftParenthesis);
		
		this.Visit(expression.Left);

		WriteLogicalOperator(expression.Operator);

		this.Visit(expression.Right);

		WriteTokenIfReq(expression, Token.RightParentThesis);

		return expression;
	}

Here one interesting thing, we may want to include the grouping parenthesis only for nested LogicalExpression. Therefore WriteTokenIfReq is written in this way:

	private void WriteTokenIfReq(Ast.LogicalExpression expression, Token token)
	{
		if (**expression.IsChild**)
		{
		    WriteToken(token);
		}
	}

Followingly, we visit BinaryExpression

	public override Ast.Expression VisitBinaryExpression(Ast.BinaryExpression expression)
	{
		this.Visit(expression.Left);
		writer.Write(GetBinaryOperator(expression.@operator));
		this.Visit(expression.Right);

		return expression;
	}

This leads to the Member and Value parsing. You can do things like

book.Id == "1"
book.Id == GetId();

,etc

You dont have to bother what kind user has specified, everthing will be parse and always return as MemberExpression and LiteralExpression

You will visit MemberEpxression to print the member:

	public override Ast.Expression VisitMemberExpression(Ast.MemberExpression expression)
	{
		writer.Write(expression.FullName);
		return expression;
	}

Here i am printing the full member name includeing the typename , of course the NameAttribute will be applied here as well.

However, MemberExpression few other useful members as well.

	MemberEpxression
		Name 
		FullName - Include the typename as well
		Member - LinqExtender.MemberReference
		DeclaringType - TypeReference
		FindAttribute<T>()
	

Final step is to write the logic for VisitLiteralExpression

	public override Ast.Expression VisitLiteralExpression(Ast.LiteralExpression expression)
	{
		WriteValue(expression.Type, expression.Value);
		return expression;
	}
 	
Here , expression.Type referes to the TypeReference of the value of returnType of the method that is compared in query


Once the query is run you will find an output similar:
	
	select * from Book
	where
	Book.Id = 10 OR (Book.Id = 1 AND Book.Author = "Charlie")

This is sample provider is included in LinqExtender.Tests project with addtional example, like how i have to visit OrderByExpression, I left that to the reader.


The project is a revamp of the original LinqExtender project at [CodePlex](http://linqExtender.codeplex.com). You can find the download and source link at the top.

Hope that helps


