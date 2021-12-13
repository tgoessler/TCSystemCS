dotnet msbuild TCSystem.sln -t:Rebuild,Pack,NugetPush -p:Configuration=Release

pause