nuget restore src\Exercise.sln
msbuild src\Helpers.Common\Helpers.Common.csproj
msbuild src\DataProviders\DataProviders.csproj
msbuild src\AddressProcessor\AddressProcessing.csproj
msbuild src\AddressProcessor.Tests\AddressProcessing.Tests.csproj
