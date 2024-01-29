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
    ServerObject(std::string configuration_path);
private:
    void HandleClient(int clientSocket);

    std::string configuration_path;
    std::map<std::string, std::string> properties;
    
    std::vector<int> clients;
};

#endif // SERVER_HPP