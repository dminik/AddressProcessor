nuget restore src\Exercise.sln
msbuild src\AddressProcessor\Helpers.Common.csproj
msbuild src\AddressProcessor\DataProviders.csproj
msbuild src\AddressProcessor.Tests\AddressProcessing.csproj
msbuild src\AddressProcessor.Tests\AddressProcessing.Tests.csproj
