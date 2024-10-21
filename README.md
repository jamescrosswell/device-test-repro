# NSubstitute mocking internal members of Trimmable assemblies 

This repository contains a minimal reproducible example of an issue we're seeing in the Sentry repository where some of 
our tests are failing when run on android and ios. 

Tests that use NSubstitute to mock internal members of Trimmable assemblies fail when run on iOS and Android, with the 
following error:

> System.ArgumentException : Can not create proxy for type Sentry.IPing because it is not accessible. Make it public, or internal and mark your assembly with [assembly: InternalsVisibleTo(\"DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7\")] attribute, because assembly Sentry is strong-named. Arg_ParamName_Name, additionalInterfacesToProxy]]

If we set `<IsTrimmable>false</IsTrimmable>` in `src/Sentry/Sentry.csproj` then the test pass.

# Running the tests

To reproduce the issue, you'll need to
- Install .NET SDK 8.0 or later
- Install Powershell 7.0 or later
- Restore the workloads for the solution (`dotnet workload restore`)
- Restore the nuget packages for the solution (`dotnet restore`)
- Start an Android emulator (I used a Pixel 5 running Android 13.0 Tiramisu | arm64 and API level 33)
- Run the device tests (`pwsh scripts/device-test.ps1 android`)
