

# Oak

Searchable database object dependency visualizer tool for MS SQL Server.

![Oak screenshot](http://kallotec.io/assets/images/projects/oak.png "Oak screenshot")

## Getting started

In Web.config, update the connection string called `db` with a connection string to the database you'd like to analyze. And check that the cache time AppSettings value `cacheTimeInMinutes` is suitable.

## Common sense

Consider using a sql login with minimal permissions in this connection string. And it should go without saying this tool shouldn't be visible from outside of your local intranet.

## Roadmap
- [ ] performance: caching of schema data
- [ ] query option: return first-level nodes only 
- [ ] definitions on double-click of tables/views
- [ ] tables and views in autocomplete list
