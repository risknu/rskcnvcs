#ifndef LOGGER_HPP
#define LOGGER_HPP

#include <iostream>
#include <string>

class Logger {
public:
    static void log(const std::string& message_type, const std::string& message, std::ostream& output = std::cout);
};

#endif // LOGGER_HPP
