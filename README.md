# Minimal reproducible example

This repository is fork of the Sentry SDK for .NET with almost everything removed except a couple of files necessary
to reproduce an issues we're having running Device Tests on xHarness that use NSubstitute since bumping the device tests
to target net8.0-android and net8.0-ios (from net7.0-android/ios). All of these tests are passing in our main branch, 
where the device test runner targets net7.0. 

On any branch where we bump to net8.0 targets for our device tests, we get the following error:

> System.ArgumentException : Can not create proxy for type Sentry.IPing because it is not accessible. Make it public, or internal and mark your assembly with [assembly: InternalsVisibleTo(\"DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7\")] attribute, because assembly Sentry is strong-named. Arg_ParamName_Name, additionalInterfacesToProxy]]

## Cause of the problem

The problem appears to be due to the `<IsTrimmable>true</IsTrimmable>` MS Build property in `src/Sentry/Sentry.csproj`.

If we set `<IsTrimmable>false</IsTrimmable>` instead, then the tests pass.

# Running the tests

To reproduce the issue, you'll need to
- Restore the workloads for the solution (`dotnet workload restore`)
- Restore the nuget packages for the solution (`dotnet restore`)
- Start an Android emulator. 
  - I've used a few different emulators but mostly a Pixel 5 running Android 13.0 Tiramisu | arm64 and Android API level 33. 
- Run the device tests (`pwsh scripts/device-test.ps1 android`)
  - You'd obviously need to have powershell installed to do that. Alternatively you can read that script to see what it's doing and just run the commands manually... all it's doing is a dotnet build and then running xharness with the appropriate parameters.  

Note that last script will both build `test/Sentry.Maui.Device.TestApp/Sentry.Maui.Device.TestApp.csproj` and run this
with xHarness, by default. It's possible to pass an additional `-Build` or `-Run` flag to only build or only run the 
tests... since the build takes a little while, that can sometimes be preferable (if a new build is not necessary).
