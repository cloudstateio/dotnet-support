# Getting started

## Prerequisites

### .Net Core SDK version
Cloudstate .Net support requires .Net Core sdk $cloudstate.dotnet.version$.

## Creating a main function

Your main class will be responsible for creating the Cloudstate gRPC server, registering the entities for your placement and starting it:

@@snip [Program.cs]($base$/docs/src/test/eventsourced/Program.cs) { #main }

We will see more details on creating entities in the coming pages.