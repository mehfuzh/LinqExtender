LinqExtender - toolkit for creating custom LINQ providers.
=============================================================

Build Requirments
===================
Framework target : .net 3.5 or better.



How to Get started
=======================

You will need a unit runner like TestDriven.Net or Galieo to get things running. You can start with the test project that contains few samples on how you can leverage the simplified expression tree to 
produce expected meta expression as you build your provider.

There is a ExpreesionTreeVisitor class (supplied in LinqExtender.Tests) that you can include in your application along with LinqExtender.dll. This visitor class is used for walking the LinqExtender tree.

The tree model is simplied in the folllowing way:

	BlockExpression
		var query = from book in context
			where book.Id  = 1
			select book


For the above query block expression will contain:

	1. TypeExpression
	2. LambdaExpression
	

For complex query like the one below:

var query = from book in context
		where (book.Id > 1) && (book.Author == "Scott" || book.Author == "John")
		Select book
		Order by book.Name asc

It will produce expression similar to:

	1. TypeExpression
	2. LambdaExpression - Where clause
		2.1  LogicalExpression - Groupings ( Left , Right)
			2.1.1 Logical.Left - BinaryExpression
			2.1.2 Logical.Right - LogicalExpression
	3. OrderByExpression
	


To start building a provider, all you need to do is to implement the following interface:

IQueryContext<T>
	 IEnumerable<T> Execute(Ast.Expression expression);


Thus, traverse the visitor. The visitor has a lot of handy getters and methods to speed things up.

You can find more reference at http://weblogs.asp.net/mehfuzh. 


Hope that helps,
Mehfuz





