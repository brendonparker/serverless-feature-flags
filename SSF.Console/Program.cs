
using SFF.Lambdas.Auth;

var generator = new ApiKeyGenerator();
var key = await generator.GenerateAsync("demo");

var res = await new ApiKeyValidation().ValidateAsync(key);

Console.WriteLine($"API KEY: {key}");
Console.WriteLine($"Valid: {res != null}");

