

# Oak

Searchable database object dependency visualizer tool for MS SQL Server.

## Warning

This initially integrated with a Neo4j graph database to cache the results, but since that functionality was not completed, it was not included. This means, in it's current state, each search is extremely inefficient.

Please first consider implementing a caching mechanism before pointing this at a production server. A caching may be committed at some point in the future. Feel free to send pull requests if you implement your own before then.

## Getting started

In Web.config, update the connection string called `db` with a connection string to the database you'd like to analyze.

## How it works

Utilizes the built-in Sql Server `sys.objects` and `sys.sql_expression_dependencies` to build dependency graphs.
