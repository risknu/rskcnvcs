#include "include/server.hpp"

std::map<std::string, std::string> ReadProperties(const std::string& filename) {
    std::map<std::string, std::string> properties;
    std::ifstream file(filename);
    if (file.is_open()) {
        std::string line;
        while (std::getline(file, line)) {
            std::istringstream iss(line);
            std::string key, value;
            if (std::getline(iss, key, '=') && std::getline(iss, value)) {
                properties[key] = value;
            }
        }
        file.close();
    } else {
        std::cout << "Can't load '" << filename << "'\n";
    }
    return properties;
}

ServerObject::ServerObject(std::string configuration_path) :
    configuration_path(configuration_path) {
        properties = ReadProperties(configuration_path);

        int server_socket = socket(AF_INET, SOCK_STREAM, 0);
        if (server_socket == -1) {
            std::cerr << "Error creating server socket" << std::endl;
        }
        sockaddr_in server_address;
        server_address.sin_family = AF_INET;
        if (inet_pton(AF_INET, properties["host_name"].c_str(), &server_address.sin_addr) <= 0) {
            std::cerr << "Invalid address/ Address not supported" << std::endl;
            close(server_socket);
        }
        server_address.sin_port = htons(std::stoi(properties["port"]));

        if (bind(server_socket, reinterpret_cast<struct sockaddr*>(&server_address), sizeof(server_address)) == -1) {
            std::cerr << "Error binding server socket" << std::endl;
            close(server_socket);
        }

        if (listen(server_socket, SOMAXCONN) == -1) {
            std::cerr << "Error listening for connections" << std::endl;
            close(server_socket);
        }

        while (true) {
            sockaddr_in client_address;
            socklen_t client_address_size = sizeof(client_address);
            int client_socket = accept(server_socket, reinterpret_cast<struct sockaddr*>(&client_address), &client_address_size);
            if (client_socket == -1) {
                std::cerr << "Error accepting connection" << std::endl;
                continue;
            }
            clients.push_back(client_socket);
            std::thread client_thread([this, client_socket] { HandleClient(client_socket); });
            client_thread.detach();
        }
        close(server_socket);
    }

void ServerObject::HandleClient(int clientSocket) {
    while (true) {
        char buffer[1024];
        ssize_t bytes_received = recv(clientSocket, buffer, sizeof(buffer), 0);
        if (bytes_received <= 0) {
            break;
        }
        for (int c : clients) {
            if (c != clientSocket) {
                send(c, buffer, bytes_received, 0);
            }
        }
    }
    clients.erase(std::remove(clients.begin(), clients.end(), clientSocket), clients.end());
    close(clientSocket);
}

int main() {
    ServerObject server("server.properties");
    return 0;
}
