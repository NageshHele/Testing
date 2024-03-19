.PHONY: hello build run compile

hello:
	@echo "Hello"

build:
	@rm -f main
	@go build -o main main.go

run:
	@go run main.go

compile:
	@echo "Compiling for every OS and Platform"
	@GOOS=linux GOARCH=386 go build -o bin/main-linux-386 main.go
	@GOOS=windows GOARCH=386 go build -o bin/main-windows-386 main.go
	
