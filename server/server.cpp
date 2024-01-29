#include "include/server.hpp"
#include "include/logger.hpp"
#include "include/utils/fileutil.hpp"

#include <cstring>
#include <netinet/in.h>
#include <sys/socket.h>
#include <arpa/inet.h>
#include <unistd.h>

ServerObject::ServerObject(const std::string host_name, const int port, std::ofstream& log_file) :
    host_name(host_name), port(port), log_file(log_file) {
        int server_socket = socket(AF_INET, SOCK_STREAM, 0);
        if (server_socket == -1) {
            Logger::log("CRTE_ERROR", "Error creating server socket", log_file);
        }
        sockaddr_in server_address;
        server_address.sin_family = AF_INET;
        if (inet_pton(AF_INET, host_name.c_str(), &server_address.sin_addr) <= 0) {
            Logger::log("INVL_ERROR", "Invalid address/ Address not supported", log_file);
            close(server_socket);
        }
        server_address.sin_port = htons(port);

        if (bind(server_socket, reinterpret_cast<struct sockaddr*>(&server_address), sizeof(server_address)) == -1) {
            Logger::log("BIND_ERROR", "Error binding server socket", log_file);
            close(server_socket);
        }

        if (listen(server_socket, SOMAXCONN) == -1) {
            Logger::log("LSTN_ERROR", "Error listening for connections", log_file);
            close(server_socket);
        }
        Logger::log("BIND_COPLETED", "The server has been successfully launched and is listening for connections to it", log_file);
        while (true) {
            sockaddr_in client_address;
            socklen_t client_address_size = sizeof(client_address);
            int client_socket = accept(server_socket, reinterpret_cast<struct sockaddr*>(&client_address), &client_address_size);
            if (client_socket == -1) {
                Logger::log("ACCP_ERROR", "Error accepting connection", log_file);
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
    clients.erase(std::remove_if(clients.begin(), clients.end(), [clientSocket](int c) {
        return c == clientSocket;
    }));
    close(clientSocket);
}

int main(int argc, char *argv[]) {
    int port = 1114;
    std::string ip = "127.0.0.1";
    if (argc >= 3) {
        for (int i = 1; i < argc; i += 2) {
            if (i + 1 < argc) {
                if (std::strcmp(argv[i], "-port") == 0) {
                    port = std::atoi(argv[i + 1]);
                } else if (std::strcmp(argv[i], "-ip") == 0) {
                    ip = argv[i + 1];
                } else {
                    std::cerr << "Unknown flag: " << argv[i] << std::endl;
                    return 1;
                }
            } else {
                std::cerr << "Error: Flag " << argv[i] << " requires a value." << std::endl;
                return 1;
            }
        }
    } else {
        std::cerr << "Usage: " << argv[0] << " -port <port_number> -ip <ip_address>" << std::endl;
        return 1; 
    }
    const std::string logFileName = "logs/s_log.txt";
    if (!fileExists(logFileName)) {
        std::ofstream createFile(logFileName);
        if (createFile.is_open()) {
            createFile.close();
            Logger::log("LOGS_COMPLETED", "The log file/folder has been created, logs are being recorded", createFile);
        } else {
            return 1;
        }
    }
    std::ofstream logs_file("logs/s_log.txt");
    ServerObject server(ip, port, logs_file);
    return 0;
}
