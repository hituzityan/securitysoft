#include <stdio.h>
#include <string.h>
#include <stdbool.h>
#include <ctype.h>

bool is_valid_email_c(const char *email) {
    if (email == NULL || strlen(email) == 0) {
        return false;
    }

    int at_count = 0;
    int dot_after_at = 0;
    int len = strlen(email);

    for (int i = 0; i < len; i++) {
        if (email[i] == '@') {
            at_count++;
            if (i > 0 && email[i - 1] == '.') return false; // @の直前に.はNG
        } else if (email[i] == '.') {
            if (at_count > 0 && i > 0 && email[i - 1] == '@') return false; // @の直後に.はNG
            if (at_count > 0) dot_after_at++;
        } else if (!isalnum(email[i]) && email[i] != '_' && email[i] != '-' && email[i] != '.') {
            return false; // 英数字、._- 以外はNG
        }
    }

    return at_count == 1 && dot_after_at > 0;
}

int main() {
    const char *email1 = "test@example.com";
    const char *email2 = "invalid-email";
    const char *email3 = "another@sample.net";

    printf("%s is %s\n", email1, is_valid_email_c(email1) ? "valid" : "invalid");
    printf("%s is %s\n", email2, is_valid_email_c(email2) ? "valid" : "invalid");
    printf("%s is %s\n", email3, is_valid_email_c(email3) ? "valid" : "invalid");

    return 0;
}
