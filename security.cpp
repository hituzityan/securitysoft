#include <iostream>
#include <string>
#include <regex>

bool is_valid_email_cpp(const std::string& email) {
    const std::regex pattern(R"([^@]+@[^@]+\.[^@]+)");
    return std::regex_match(email, pattern);
}

int main() {
    std::string email1 = "test@example.com";
    std::string email2 = "invalid-email";
    std::string email3 = "another@sample.net";

    std::cout << email1 << " is " << (is_valid_email_cpp(email1) ? "valid" : "invalid") << std::endl;
    std::cout << email2 << " is " << (is_valid_email_cpp(email2) ? "valid" : "invalid") << std::endl;
    std::cout << email3 << " is " << (is_valid_email_cpp(email3) ? "valid" : "invalid") << std::endl;

    return 0;
}
