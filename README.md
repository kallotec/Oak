# Oak

Searchable MS SQL Server database object dependency visualizer tool.

![Oak screenshot](http://kallotec.io/assets/images/projects/oak.png "Oak screenshot")

## Getting started

In Web.config, update the connection string called `db` with a connection string to the database you'd like to analyze.

## Roadmap

- [ ] performance improvements for large schemas
- [x] nice autocomplete gui showing object type, incl filter by type options
- [x] walk direction selection: be able to see what an object references, or what objects reference it
- [x] render direction: render as vertical or horizontal tree
- [x] definitions on double-click of tables/views
- [x] tables and views in autocomplete list
