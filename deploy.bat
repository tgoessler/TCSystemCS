dotnet msbuild TCSystem.slnx -t:Rebuild,Pack,NugetPush -p:Configuration=Release

pause