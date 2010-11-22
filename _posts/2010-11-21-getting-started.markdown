---
layout : default
title : Gettting Started
---

# A simple Text provider using LinqExtender. #

The goal of this post is to show how you can get started creating custom provider using LinqExtender.


LinqExtender provides an entry-point interface that is invoked with populated simplified expression tree, visiting which the expected intermidate request or query can be built and used to product the result for a given LINQ query.

Moreover, It abstracts the complexity of intializing the query provider, doing projection, parsing method calls and extracting  value from original System.Linq.Expresssion thus speeds up the creation of custom provider.

<pre class="brush:csharp">

<code>
public interface IQueryContext<T>
{
  IEnumerable<T> Execute(Ast.Expression expression);  
}

</code>

</pre>

Unfininshed....