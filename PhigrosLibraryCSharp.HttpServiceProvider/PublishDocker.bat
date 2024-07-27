rem i had to do these becase visual studio keep fuck me up

rem C:\Program Files\dotnet\dotnet.exe 
rem "C:\Program Files\dotnet\sdk\8.0.302\Containers\build\..\containerize\containerize.dll" 
rem C:\Users\yt698\source\repos\PhigrosLibraryCSharp\PhigrosLibraryCSharp.HttpServiceProvider\obj\Release\net8.0\linux-x64\PubTmp\Out 
rem --baseregistry mcr.microsoft.com 
rem --baseimagename dotnet/aspnet 
rem --repository phigroslibrarycsharp-httpserviceprovider 
rem --workingdirectory /app 
rem --baseimagetag 8.0 
rem --outputregistry registry.hub.docker.com 
rem --appcommand dotnet PhigrosLibraryCSharp.HttpServiceProvider.dll 
rem --labels org.opencontainers.image.created=2024-07-22T05:05:10.2535899Z org.opencontainers.artifact.created=2024-07-22T05:05:10.2535899Z org.opencontainers.image.authors=PhigrosLibraryCSharp.HttpServiceProvider org.opencontainers.image.version=1.0.0 org.opencontainers.image.base.name=mcr.microsoft.com/dotnet/aspnet:8.0 net.dot.runtime.majorminor=8.0 net.dot.sdk.version=8.0.302 
rem --imagetags latest 
rem --rid linux-x64 
rem --ridgraphpath "C:\Program Files\dotnet\sdk\8.0.302/PortableRuntimeIdentifierGraph.json" 
rem --generate-labels 
rem --generate-digest-label 

"C:\Program Files\dotnet\dotnet.exe" "C:\Program Files\dotnet\sdk\8.0.302\Containers\build\..\containerize\containerize.dll" C:\Users\yt698\source\repos\PhigrosLibraryCSharp\PhigrosLibraryCSharp.HttpServiceProvider\obj\Release\net8.0\linux-x64\PubTmp\Out --baseregistry mcr.microsoft.com --baseimagename dotnet/aspnet --repository yt6983138/phigroslibrarycsharp-httpserviceprovider --workingdirectory /app --baseimagetag 8.0 --outputregistry registry.hub.docker.com --appcommand dotnet PhigrosLibraryCSharp.HttpServiceProvider.dll --labels org.opencontainers.image.created=2024-07-22T05:05:10.2535899Z org.opencontainers.artifact.created=2024-07-22T05:05:10.2535899Z org.opencontainers.image.authors=PhigrosLibraryCSharp.HttpServiceProvider org.opencontainers.image.version=1.0.0 org.opencontainers.image.base.name=mcr.microsoft.com/dotnet/aspnet:8.0 net.dot.runtime.majorminor=8.0 net.dot.sdk.version=8.0.302 --imagetags latest --rid linux-x64 --ridgraphpath "C:\Program Files\dotnet\sdk\8.0.302/PortableRuntimeIdentifierGraph.json" --generate-labels --generate-digest-label 