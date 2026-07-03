# LogicBuilder.App.Bsl.Utils

[![CI](https://github.com/BpsLogicBuilder/LogicBuilder.App.Bsl.Utils/actions/workflows/ci.yml/badge.svg)](https://github.com/BpsLogicBuilder/LogicBuilder.App.Bsl.Utils/actions/workflows/ci.yml)
[![CodeQL](https://github.com/BpsLogicBuilder/LogicBuilder.App.Bsl.Utils/actions/workflows/github-code-scanning/codeql/badge.svg)](https://github.com/BpsLogicBuilder/LogicBuilder.App.Bsl.Utils/actions/workflows/github-code-scanning/codeql)
[![codecov](https://codecov.io/gh/BpsLogicBuilder/LogicBuilder.App.Bsl.Utils/graph/badge.svg?token=QSA4HJTQXI)](https://codecov.io/gh/BpsLogicBuilder/LogicBuilder.App.Bsl.Utils)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=BpsLogicBuilder_LogicBuilder.App.Bsl.Utils&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=BpsLogicBuilder_LogicBuilder.App.Bsl.Utils)

A .NET library that provides utilities for handling query requests with dynamic filters, expansions, and projections for Entity Framework Core applications. This library enables type-safe, expression-based querying with support for both generic and non-generic scenarios, making it ideal for building flexible data access layers in enterprise applications.

## Features

- **Dynamic Query Building**: Construct complex LINQ queries using expression descriptors
- **Type-Safe Operations**: Generic methods ensure compile-time type safety while supporting runtime type resolution
- **Filter & Select Operations**: Build and execute filtered queries with custom select projections
- **Expand/Include Support**: Dynamically include related entities using SelectExpandDefinition
- **Anonymous Type Support**: Query and return dynamic/anonymous types for flexible data transfer objects
- **Entity Retrieval**: Retrieve single entities with filtering and expansion capabilities
- **List Queries**: Execute queries that return collections with flexible return types (IEnumerable, IQueryable, etc.)
- **Delete Operations**: Execute delete operations using dynamic filter expressions
- **Persistence Operations**: Synchronous wrappers for common Entity Framework Core CRUD operations
- **Entity Framework Core**: Built on top of Entity Framework Core for robust data access

## Core Components

### IRequestHelper Interface

The main interface providing three primary operations:

- **GetEntity**: Retrieve a single entity based on a filter expression
- **GetList**: Retrieve a typed list of entities with custom projections
- **GetAnonymousList**: Retrieve a list of dynamic objects for anonymous type projections

Each method supports both generic and non-generic overloads for maximum flexibility.

### Operation Interfaces

#### IDeleteOperations
Provides delete operations using parameter-based filter expressions:
- **Delete<TModel, TData>**: Delete entities matching a FilterLambdaOperatorParameters expression

#### IProjectionOperations
Provides entity retrieval with filtering, querying, and expansion support:
- **Get<TModel, TData>**: Retrieve a single entity with optional filtering, query functions, and expansion
- **GetItems<TModel, TData>**: Retrieve a collection of entities with optional filtering, query functions, and expansion

#### IQueryOperations
Provides advanced query operations with flexible return types:
- **Query<TModel, TData, TModelReturn, TDataReturn>**: Execute custom LINQ queries with mapping between model and data types

### Static Operation Classes

#### PersistenceOperations<TModel, TData>
Provides synchronous wrappers for Entity Framework Core persistence operations:
- **AddChange**: Add or update a single entity
- **AddChanges**: Add or update multiple entities
- **AddGraphChange**: Add or update an entity graph (with related entities)
- **AddGraphChanges**: Add or update multiple entity graphs
- **Delete**: Delete entities matching a filter expression
- **Save**: Save a single entity
- **SaveList**: Save multiple entities
- **SaveGraph**: Save an entity graph
- **SaveGraphs**: Save multiple entity graphs

#### DeleteOperations<TModel, TData>
Static wrapper for delete operations:
- **Delete**: Execute delete operations using FilterLambdaOperatorParameters

#### ProjectionOperations<TModel, TData>
Static wrappers for projection operations:
- **Get** (GetSingle): Retrieve a single entity synchronously
- **GetItems** (GetList): Retrieve a collection of entities synchronously

#### QueryOperations<TModel, TData, TModelReturn, TDataReturn>
Static wrapper for query operations:
- **Query**: Execute custom queries with flexible return types synchronously

All static operation classes use the `[AlsoKnownAs]` and `[FunctionGroup]` attributes for integration with the LogicBuilder framework, enabling these methods to be called from business rules and workflows.

### Request Types

- **GetEntityRequest**: Defines entity retrieval with filter and optional select/expand definitions
- **GetTypedListRequest**: Defines list queries with selector expressions and optional expansions
- **GetObjectListRequest**: Defines queries returning dynamic/anonymous objects

### Response Types

- **GetEntityResponse**: Contains the retrieved entity and success status
- **GetListResponse**: Contains the retrieved list and success status
- **GetObjectListResponse**: Contains dynamic objects and success status

## Use Cases

- Building flexible Web APIs with OData-like query capabilities
- Implementing repository patterns with dynamic query construction
- Creating data access layers that support runtime type resolution
- Developing business logic layers that require expression-based filtering and projection
- Building applications that need to query data using externally-defined expression descriptors
- Executing persistence operations (CRUD) with type-safe generic methods
- Integrating data operations with rule-based business logic systems
- Performing complex entity graph operations (saving entities with related data)

## Dependencies

- **LogicBuilder.App.Bsl.Business**: Business layer components for requests and responses
- **LogicBuilder.App.Common.Utils**: Common utilities for mapping operations
- **LogicBuilder.EntityFrameworkCore**: EF Core extensions and repository patterns
- **LogicBuilder.Attributes**: Attributes for function metadata and framework integration
- **.NET 10.0**: Built for the latest .NET platform

## Example Scenarios

The library handles complex scenarios such as:

- Querying departments with related courses using expand definitions
- Selecting and projecting entities to different model types (e.g., Department → LookUpsModel)
- Ordering, filtering, and limiting results dynamically
- Grouping data with aggregations (e.g., group students by enrollment date with counts)
- Type-safe mapping between entity types and model types using expression descriptors
- Deleting entities based on dynamic filter expressions
- Saving entity graphs with related entities in a single operation
- Executing synchronous CRUD operations from business rules

## Target Framework

- .NET 10.0

## License

MIT License - Copyright © BPS 2026

## Repository

[https://github.com/BpsLogicBuilder/LogicBuilder.App.Bsl.Utils](https://github.com/BpsLogicBuilder/LogicBuilder.App.Bsl.Utils)

## Related Projects

- [LogicBuilder](https://github.com/BpsLogicBuilder/LogicBuilder)
- [LogicBuilder.App.Common.Utils](https://github.com/BpsLogicBuilder/LogicBuilder.App.Common.Utils)