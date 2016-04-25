

# Oak

Searchable database object dependency visualizer tool for MS SQL Server.

## Getting started

In Web.config, update the connection string called `db` with a connection string to the database you'd like to analyze. And check that the cache time AppSettings value `cacheTimeInMinutes` is suitable.

## Common sense

Consider using a sql login with minimal permissions in this connection string. And it should go without saying this tool shouldn't be visible from outside of your local intranet.
