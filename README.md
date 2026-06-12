# Unit Conversion API

A small ASP.NET Core Web API that converts numeric values between units of
measurement. Out of the box it covers three categories:

- **Length** - meters, kilometers, centimeters, millimeters, miles, yards, feet, inches
- **Mass** - kilograms, grams, milligrams, pounds, ounces, stone
- **Temperature** - Celsius, Fahrenheit, Kelvin

## Running it locally

You'll need the [.NET 8 SDK](https://dotnet.microsoft.com/download).

```bash
# from the repo root
dotnet restore
dotnet run --project src/UnitConverter.Api
```

By default the API listens on `http://localhost:5080`. With the project
running in the `Development` environment (the default for `dotnet run`),
Swagger UI is available at:

```
http://localhost:5080/swagger
```

That's the easiest way to try out the endpoints without needing a separate
HTTP client.

### Running the tests

```bash
dotnet test
```

## API overview

### `POST /api/v1/conversions`

Converts a value from one unit to another. Both units must belong to the
same category.

Request body:

```json
{
  "category": "length",
  "from": "m",
  "to": "ft",
  "value": 10
}
```

Response:

```json
{
  "category": "Length",
  "from": "m",
  "to": "ft",
  "inputValue": 10,
  "convertedValue": 32.8084
}
```

If the category or either unit code isn't recognised, the API responds with
`400 Bad Request` and a `ProblemDetails` body explaining what was wrong.

### `GET /api/v1/units`

Returns every category along with the unit codes and display names
available in it. Handy for populating a "from"/"to" dropdown on the client
side without hardcoding the list.

### `GET /api/v1/units/{category}`

Same as above, but scoped to a single category (`length`, `mass` or
`temperature`).

## Project structure

```
src/UnitConverter.Api/
  Controllers/      API endpoints
  Models/           Request/response DTOs and the UnitDefinition/UnitCategory types
  Services/         Conversion logic + the unit catalog
  Exceptions/       Domain-specific exceptions
  Middleware/        Translates those exceptions into 400 responses
  Program.cs         App startup / DI wiring

tests/UnitConverter.Api.Tests/
  Unit tests for the conversion logic
```

## Design decisions & trade-offs

**Conversions go through a base unit per category.** Rather than storing a
conversion factor for every unit-to-unit pair, each unit only knows how to
convert to and from a single "base" unit for its category (meters for
length, kilograms for mass, Celsius for temperature). A conversion is just
`from -> base -> to`. This keeps the data small and means adding a new unit
is a one-line addition, not an `O(n^2)` table update.

**Temperature uses functions, not just multipliers.** Length and mass
conversions are pure scale factors, but Fahrenheit and Kelvin both involve
an additive offset as well. Rather than special-casing temperature in the
service, every unit defines a `ToBase`/`FromBase` pair (`Func<double,
double>`). For most units these are just `x * factor` and `x / factor`, but
it means the conversion logic itself doesn't need to know or care that
temperature is "different".

**The unit catalog sits behind `IUnitCatalog`.** The current implementation
hardcodes the units in `UnitCatalog.cs`, as allowed by the brief. The
brief also mentions this should eventually support hundreds of units and
conversion types - when that happens, the data would move to a database or
configuration file, and only `UnitCatalog` would need to change. The
service and controllers are written against the `IUnitCatalog` interface
and don't know where the data comes from.

**Validation errors vs. server errors.** Unknown categories or unit codes
are treated as client errors (`400`, via a small `ConversionException`
hierarchy and a piece of middleware that turns them into
`ProblemDetails`), rather than bubbling up as `500`s. This felt closer to
how the API would behave in a real product, where "the user typed `kgg`
instead of `kg`" is an everyday occurrence, not an exceptional one.

**What I'd add next**, given more time: pagination/filtering on the units
endpoint once the list gets large, request logging/correlation IDs, and
probably an integration test that spins up the API with
`WebApplicationFactory` and hits it over HTTP rather than only unit-testing
the service layer directly.
