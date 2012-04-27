LinqExtender - toolkit for creating custom Linq providers.
=============================================================

Build Requirments
===================
Framework target : .net 3.5 or better.



How to Get started
=======================

You will need a unit runner like TestDriven.Net or Galieo to get things running. The LinqExtender.Tests project has few samples that shows how you can use its simplified expression tree to 
produce expected target query or request while building your provider.

There is a ExpreesionTreeVisitor class (supplied in LinqExtender.Tests) that you can include in your application along with LinqExtender.dll.This tree visitor walks the LinqExtender tree.

The tree model is simplied in the folllowing way:

	BlockExpression
		var query = from book in context
			where book.Id  = 1
			select book


For the above query block expression will contain:

	1. TypeExpression
	2. LambdaExpression
	

For complex query like the one shown below:

var query = from book in context
		where (book.Id > 1) && (book.Author == "Scott" || book.Author == "John")
		Select book
		Order by book.Name asc

The block expression will contain:

	1. TypeExpression
	2. LambdaExpression - Where clause
		2.1  LogicalExpression - Groupings ( Left , Right)
			2.1.1 Logical.Left - BinaryExpression
			2.1.2 Logical.Right - LogicalExpression
	3. OrderByExpression
	


To build a provider all you need to do is to implement the following interface:

IQueryContext<T>
	 IEnumerable<T> Execute(Ast.Expression expression);


Thus, travase the visitor. The visitor has lots of handy getters and method calls to speed things up.

More will be documented in the my blog at http://weblogs.asp.net/mehfuzh. Also, this is a revamp of
existing linqextender at linqextender.codeplex.com

Hope that helps,
Mehfuz





