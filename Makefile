dummy:
	@echo run-operator
	@echo build-function
	@echo run-function
	@echo build-js-function
	@echo run-js-function

run-operator:
	docker run -it --rm --name cloudstate -p 9000:9000 cloudstateio/cloudstate-proxy-dev-mode

build-function:
	docker build -f Dockerfile.csharp-shopping-cart -t cloudstateio/csharp-shopping-cart:latest .

run-function:
	docker run -it --rm --network container:cloudstate --name shopping-cart -e "DEBUG=cloudstate*" cloudstateio/csharp-shopping-cart:latest

build-js-function:
	cd ../cloudstate/samples/js-shopping-cart && npm run dockerbuild

run-js-function:
	docker run -it --rm --network container:cloudstate --name shopping-cart -e "DEBUG=cloudstate*" cloudstateio/js-shopping-cart:latest

test-function:
	grpc_cli ls localhost:9000 -l

test-additem:
	grpc_cli call localhost:9000 com.example.shoppingcart.ShoppingCart.AddItem 'user_id: "test", product_id: "p123", name: "pthing", quantity: 100'

test-getcart:
	grpc_cli call localhost:9000 com.example.shoppingcart.ShoppingCart.GetCart 'user_id: "test"'
