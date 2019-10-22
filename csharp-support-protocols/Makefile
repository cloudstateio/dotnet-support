PROTO_DIR=./proto_files

OUT_DIR=./protocols
GRPC_DIR=./protocols/grpc

PROTO_PATH_FLAGS=--proto_path=${PROTO_DIR}

# TODO: need to sort out the issues with Grpc.Tools / Protobuf
WITH_SERVER=--plugin=protoc-gen-grpc=~/.nuget/packages/grpc.tools/2.23.0/tools/macosx_x64/grpc_csharp_plugin --grpc_out=${GRPC_DIR}

OUT_FLAGS=--csharp_out=${OUT_DIR}

OBJECTS = ${OUT_DIR}/EntityKey.cs ${OUT_DIR}/Http.cs 
OBJECTS += ${OUT_DIR}/Crdt.cs ${OUT_DIR}/Entity.cs ${OUT_DIR}/EventSourced.cs ${OUT_DIR}/Function.cs
OBJECTS += ${GRPC_DIR}/CrdtGrpc.cs ${GRPC_DIR}/EntityGrpc.cs ${GRPC_DIR}/EventSourcedGrpc.cs ${GRPC_DIR}/FunctionGrpc.cs
OBJECTS += ${GRPC_DIR}/ShoppingCartGrpc.cs ${OUT_DIR}/ShoppingCart.cs
OBJECTS += ${GRPC_DIR}/Crdt-exampleGrpc.cs ${OUT_DIR}/CrdtExample.cs
OBJECTS += ${OUT_DIR}/Annotations.cs
OBJECTS += ${OUT_DIR}/Domain.cs 
OBJECTS += ${OUT_DIR}/Empty.cs

all: $(OBJECTS)

${OUT_DIR}/Annotations.cs: ${PROTO_DIR}/google/api/annotations.proto
	protoc ${PROTO_PATH_FLAGS} $^ ${OUT_FLAGS}

${OUT_DIR}/EntityKey.cs: ${PROTO_DIR}/cloudstate/entity_key.proto
	protoc ${PROTO_PATH_FLAGS} $^ ${OUT_FLAGS}

${OUT_DIR}/Http.cs:	${PROTO_DIR}/google/api/http.proto
	protoc ${PROTO_PATH_FLAGS} $^ ${OUT_FLAGS}

${OUT_DIR}/Crdt.cs ${GRPC_DIR}/CrdtGrpc.cs: ${PROTO_DIR}/cloudstate/crdt.proto
	protoc ${PROTO_PATH_FLAGS} ${WITH_SERVER} $^ ${OUT_FLAGS}

${OUT_DIR}/Entity.cs ${GRPC_DIR}/EntityGrpc.cs: ${PROTO_DIR}/cloudstate/entity.proto
	protoc ${PROTO_PATH_FLAGS} ${WITH_SERVER} $^ ${OUT_FLAGS}

${OUT_DIR}/EventSourced.cs ${GRPC_DIR}/EventSourcedGrpc.cs: ${PROTO_DIR}/cloudstate/event_sourced.proto
	protoc ${PROTO_PATH_FLAGS} ${WITH_SERVER} $^ ${OUT_FLAGS}

${OUT_DIR}/Function.cs ${GRPC_DIR}/FunctionGrpc.cs: ${PROTO_DIR}/cloudstate/function.proto
	protoc ${PROTO_PATH_FLAGS} ${WITH_SERVER} $^ ${OUT_FLAGS}

${GRPC_DIR}/ShoppingCartGrpc.cs ${OUT_DIR}/ShoppingCart.cs: ${PROTO_DIR}/example/shoppingcart/shoppingcart.proto
	protoc ${PROTO_PATH_FLAGS} ${WITH_SERVER} $^ ${OUT_FLAGS} 

${OUT_DIR}/Domain.cs: ${PROTO_DIR}/example/shoppingcart/persistence/domain.proto
	protoc ${PROTO_PATH_FLAGS} $^ ${OUT_FLAGS}

${OUT_DIR}/CrdtExample.cs ${GRPC_DIR}/Crdt-exampleGrpc.cs: ${PROTO_DIR}/example/crdts/crdt-example.proto
	protoc ${PROTO_PATH_FLAGS} ${WITH_SERVER} $^ ${OUT_FLAGS}

${OUT_DIR}/Empty.cs: ${PROTO_DIR}/google/protobuf/empty.proto
	protoc ${PROTO_PATH_FLAGS} $^ ${OUT_FLAGS}


.PHONY: clean
clean: 
	rm $(OBJECTS) 2> /dev/null || echo > /dev/null

other:	
	# protoc ${PROTO_DIR}/proxy/grpc/reflection/v1alpha/reflection.proto ${OUT_FLAGS}
	
