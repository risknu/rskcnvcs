cmake_minimum_required(VERSION 3.10)
project(rskcnv)

set(CMAKE_CXX_STANDARD 11)

find_package(SFML 2.5 COMPONENTS graphics window system REQUIRED)

add_executable(server src/server.cpp)

target_link_libraries(server sfml-graphics sfml-window sfml-system sfml-network)
