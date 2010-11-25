---
layout : default
title : Gettting Started
---

### A simple Text provider using LinqExtender.

The goal of this post is to get started creating custom LINQ (Language Integrated Query) provider using LinqExtender.


To begin, let's say i want to build a text provider that prints the TSQL representation of the LINQ query to the screen. Thus, i will create a context class and first implement the following interface from LinqExtender namespace:

	public interface IQueryContext<T>
	{
	  IEnumerable<T> Execute(Ast.Expression expression);  
	}

The interface has only one method named `Execute` that accepts translated expression that is poplulated by the extender and which will be visited to produce the desired TSQL statement.

Generally, the expression will be traversed by visitor pattern, one can write the vistor class specific to LinqExtender then include it as an base class and override its various methods to build the expected meta that will be run against a data store or send over HTTP to produce the expected result or even in my case build a simple TSQL.

To smooth things up, a similar class is provided in LinqExtender.Tests project which i will start as a reference.

Before doing a deep dive. Let me do an short introduction on how the simiplied tree is constructed.

Let's consider the following LINQ query:

	var query = from book in context
				where book.Id  = 1
				select book

This is translated into :
	
	BlockExpression
		TypeExpression - Contains various reflected accessors for target object.
		LambdaExpression - Represents the where clause.
			BinaryExpression
				MemberExpression - Contains member related to method and accessors
				LiteralEpxression - Contains the evaluted value.

Moving forward to a bit more complex query:

	var query = from book in context
		where (book.Id > 1) && (book.Author == "Scott" || book.Author == "John")
		Select book

It is translated to:

	BlockExpression
		TypeExpression : Name == "Book", If NameAttribute applied then Name = "as specified".
		LambdaExpression
			LogicalExpresion - Contains the logical parts | Operator = LogicalOperator.AND
				BinaryExpression 
					MemberExpression 
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
									
Here , BinaryExpression or LambdaExpression is LinqExtender's version and thus all the expression contains various accessors and methods that easily let you get query information.

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
				
It will also produce the same tree as the previous one. Therefore, it turns out that projection is taken care of internally.

In my sample Text provider, output will be stored in a StringBuilder and can be then printed out to Console.

	var builder = new StringBuilder();
	var context = new TextContext<Book>(new StringWriter(builder));

	var query = from book in context
			    where book.Id == 10 
			    || (book.Id == 1 && book.Author == "Charlie")
			    select book;

	query.Count();

	Console.WriteLine(builder.ToString());				

Now, inside the `IQueryContext<T>.Execute(Ast.Expression)`, i wrote it like: 

	public IEnumerable<T> Execute(Ast.Expression expression)
	{
		this.Visit(expression);
		return new List<T>().AsEnumerable();
	}

Since here result is not important, therefore returned a new instance of `List<T>` and `this.Visit(expression)` will eventually branch to various overrides from `ExpressionVisitor` that I have included in TextContext class; once it has reached the end, the builder will contain a nice formatted TSQL.


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

If we follow the translated flow, the first expression that will be of my concern is the TypeExperession where i will be first formating "Select * From {TypeName}" string .


	public override Ast.Expression VisitTypeExpression(Ast.TypeExpression expression)
	{
		writer.Write(string.Format("select * from {0}", expression.Type.Name));
		return expression;
	}
	
Now, expression.Type is not System.Type rather its LinqExtender.TypeReference, the Name returns either the original typename or the name that user specifies on top of the class through LinqExtender.NameAttribute, let's for example take the following class:

	[Name("flickr.photos.search")]
	public class Photo
	{

	}

In this case expression.Type.Name == "flickr.photos.search"

Considering this part:
where book.Id == 10 || (book.Id == 1 && book.Author == "Charlie")

It will be translated like this

BinaryExpression
LogicalExpression
	BinaryExpression
	BinaryExpression

To print / generate the equivalant TSQL for it , we first of all not need to worry about the order in which the gropings are made or the level of nested groupings are used in the query. While visiting the expression,  in any case we only have to generate the meta for the respected expression type. It will be inovoked by extender as it is specified in query during execution.
	
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

Here one interesting thing, we may want to include the grouping parenthesis only for nested LogicalExpression. Therefore `WriteTokenIfReq` is written in this way:

	private void WriteTokenIfReq(Ast.LogicalExpression expression, Token token)
	{
		if (**expression.IsChild**)
		{
		    WriteToken(token);
		}
	}

Followingly, I override the BinaryExpression:

	public override Ast.Expression VisitBinaryExpression(Ast.BinaryExpression expression)
	{
		this.Visit(expression.Left);
		writer.Write(GetBinaryOperator(expression.@operator));
		this.Visit(expression.Right);

		return expression;
	}

This leads to the Member and Value parsing. In this case, we may how user might write his query, he can do:

book.Id == "1"
book.Id == GetId();
...
...
etc

However, we dont have to bother how as in LinqExtender BinaryExpression.Left will either be BinaryExpression or MemberExpression and BinaryExpression.Right will either be BinaryExpression or LiteralExpression.

In context of the text provider, I have to visit the MemberEpxression to print the member:

	public override Ast.Expression VisitMemberExpression(Ast.MemberExpression expression)
	{
		writer.Write(expression.FullName);
		return expression;
	}

Here i am printing the full member name includeing the typename , of course the NameAttribute will be applied here as well.

However, MemberExpression wraps in other useful accessors and methods like:

	MemberEpxression
		Name 
		FullName - Includes the type name
		Member - is LinqExtender.MemberReference
		DeclaringType - is TypeReference
		FindAttribute<T>() - Finds user-defined attribute
	

Final step is to write the logic for VisitLiteralExpression

	public override Ast.Expression VisitLiteralExpression(Ast.LiteralExpression expression)
	{
		WriteValue(expression.Type, expression.Value);
		return expression;
	}
 	
Here , expression.Type referes to the TypeReference of value or member type that is compared in where


Once the query is run it will print the output:
	
	select * from Book
	where
	Book.Id = 10 OR (Book.Id = 1 AND Book.Author = "Charlie")

This sample provider is included in LinqExtender.Tests project with addtional examples, like how i have to visit OrderByExpression, I leave that for the reader.


The project is a revamp of the original LinqExtender project at [CodePlex](http://linqExtender.codeplex.com). The source and download is included at the top. Moreover, please feel free to fork and make updates and i will be happy to merge.

Hope this helps


