#ifndef SERVER_HPP
#define SERVER_HPP

#include <string>
#include <iostream>
#include <vector>
#include <thread>
#include <cstring>
#include <unistd.h>
#include <arpa/inet.h>
#include <fstream>
#include <map>
#include <sstream>

class ServerObject {
public:
    ServerObject(const std::string host_name, const int port, std::ofstream& log_file);
private:
    void HandleClient(int clientSocket);

    const std::string host_name;
    const int port;
    std::ofstream& log_file;
    std::map<std::string, std::string> properties;
    
    std::vector<int> clients;
};

#endif // SERVER_HPP