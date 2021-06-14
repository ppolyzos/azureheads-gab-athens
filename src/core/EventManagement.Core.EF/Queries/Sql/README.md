# Stored Procedures in EF Core

## Usage

### Executing A Stored Procedure

Add the `using` statement to pull in the extension method. E.g: `using EFCoreFluent`

```csharp
var dbContext = GetDbContext();
dbContext.LoadStoredProc("dbo.SomeSproc")
    .WithSqlParam("fooId", 1)              
    .ExecuteStoredProcedure((handler) =>
    {                  
        var fooResults = handler.ReadToList<FooDto>();      
        // do something with your results.
    });
```

### Handling Multiple Result Sets

```csharp
var dbContext = GetDbContext();
dbContext.LoadStoredProc("dbo.SomeSproc")
    .WithSqlParam("fooId", 1)              
    .ExecuteStoredProcedure((handler) =>
    {                  
        var fooResults = handler.ReadToList<FooDto>();      
        handler.NextResult();

        var barResults = handler.ReadToList<BarDto>();
        handler.NextResult();
        
        var bazResults = handler.ReadToList<BazDto>()
    });
```

### Handling Output Parameters

```csharp
DbParameter outputParam = null;

var dbContext = GetDbContext();
dbContext.LoadStoredProc("dbo.SomeSproc")
    .WithSqlParam("fooId", 1)  
    .WithSqlParam("myOutputParam", (dbParam) =>
    {                 
        dbParam.Direction = System.Data.ParameterDirection.Output;
        dbParam.DbType = System.Data.DbType.Int32;          
        outputParam = dbParam;
    })
    .ExecuteStoredProcedure((handler) =>
    {                  
        var fooResults = handler.ReadToList<FooDto>();      
        handler.NextResult();
        var barResults = handler.ReadToList<BarDto>();
        handler.NextResult();
        var bazResults = handler.ReadToList<BazDto>()
    });
    
    int outputParamValue = (int)outputParam?.Value;
```
