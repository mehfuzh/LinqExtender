---
layout : default
title : Gettting Started
---

### A simple Text provider using LinqExtender.

The goal of this post is to get started creating custom provider using LinqExtender.


To begin, let's say i want to build a simple provider. the context or entry point class that will be queried upon, need to first implement the following interface:
	
    public interface IQueryContext<T>
    {
      IEnumerable<T> Execute(Ast.Expression expression);  
    }

The interface has only one method named Execute that accepts custom expression that is poplulated by the extender and which we will parsing to produce TSQL statment.

Generally, the expresion with be visited by visitor pattern, one can write the vistor class specific to LinqExtender and include as an base class and override its various methods to build the expected query / request.
However a similar class is provided in LinqExtender.Tests project to start as a reference.

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


.... continue.
				

