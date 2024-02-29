# Project Name
Dependency Resolver


## Project Description

This project aims to demonstrate resource interaction using dotnet CLI and Node.js. It clones the GitHub repository, fetches project dependencies, and creates resource interactions in JSON format with parent-child relationships.

## Prerequisites

- [git](https://git-scm.com/download)
- [dotnet CLI](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/en/download/)

## Installation

1. Clone the repository:

    ```shell
    git clone https://github.com/your-username/your-repository.git
    ```

2. Install dotnet dependencies:

    ```shell
    dotnet restore
    ```

## Run the project

To run the project, navigate to the directory where the `.csproj` file is located and execute the following command:

    ```
    dotnet run --c Release -v q "https://github.com/Azure-Samples/todo-csharp-sql"
    ```

## Usage

## Contributing

Contributions are welcome! If you find any issues or have suggestions for improvements, please open an issue or submit a pull request.

## License

This project is licensed under the [MIT License](LICENSE).