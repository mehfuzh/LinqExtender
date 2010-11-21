select * from Book
where
Book.Id = 10 OR (Book.Id = 1 AND Book.Author = "Charlie")