#include "include/logger.hpp"

void Logger::log(const std::string& message_type, const std::string& message, std::ostream& output) {
    output << "[" << message_type << "] " << message << std::endl;
    std::cout << "[" << message_type << "] " << message << std::endl;
}
