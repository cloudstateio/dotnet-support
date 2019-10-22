#!/bin/bash

# -----------
# This script will ensure dependencies are all set for building
# and updating the tooling components for cloudstate csharp impl
# -----------

#
# Build custom GRPC (Note, they accepted my PR so this isn't required anymore)
#
# if [ ! -d "./grpc" ]; then
#     git clone https://github.com/grpc/grpc.git grpc
#     pushd grpc
#     git checkout master
#     git submodule update --init
#     # Note: this might be required for GRPC
#     #brew install autoconf automake libtool shtool gflags go
#     python tools/run_tests/run_tests.py -l csharp -c dbg --build_only
#     popd
# fi

#
# Build proto source to expose internal properties
#
if [ ! -d "./protobuf" ]; then
    git clone https://github.com/protocolbuffers/protobuf.git protobuf
fi
pushd protobuf
git checkout master
sed -i '' 's/internal FileDescriptorProto Proto/public FileDescriptorProto Proto/' ./csharp/src/Google.Protobuf/Reflection/FileDescriptor.cs
popd

#
# Update cloudstate src
#
if [ ! -d "./cloudstate" ]; then
    git clone https://github.com/cloudstateio/cloudstate.git cloudstate
fi
pushd cloudstate
git checkout master
git pull
popd

#
# Copy proto files from cloudstate
#
rm -rf csharp-support-protocols/proto_files
cp -r cloudstate/protocols csharp-support-protocols/proto_files
pushd csharp-support-protocols

#
# Fix up proto files for csharp
#
sed -i '' 's/option java_package = "com.google.api";/option java_package = "com.google.api";\
option csharp_namespace = "Google.Protobuf";/' \
    ./proto_files/frontend/google/api/annotations.proto
sed -i '' 's/option java_package = "com.google.api";/option java_package = "com.google.api";\
option csharp_namespace = "Google.Protobuf";/' \
    ./proto_files/frontend/google/api/http.proto

# 
# Organize proto directory to resolve dependencies easier
#
mv ./proto_files/frontend/google ./proto_files/google
mv ./proto_files/frontend/cloudstate ./proto_files/cloudstate
rm -rf ./proto_files/frontend
mv ./proto_files/protocol/cloudstate/*.proto ./proto_files/cloudstate

#
# Add remaining proto files
#
mkdir ./proto_files/google/protobuf
curl https://raw.githubusercontent.com/protocolbuffers/protobuf/master/src/google/protobuf/empty.proto \
    > ./proto_files/google/protobuf/empty.proto

make

popd

