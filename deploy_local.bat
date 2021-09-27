dotnet msbuild TCSystem.sln -t:Rebuild,Pack,NugetCopy -p:Configuration=Release

pause