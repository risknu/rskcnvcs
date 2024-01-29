#include "../include/utils/fileutil.hpp"

bool fileExists(const std::string& filename) {
    std::ifstream file(filename);
    return file.good();
}
