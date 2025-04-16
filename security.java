import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

import java.util.Arrays;
import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

@SpringBootApplication
public class ProtectEmailApplication {

    public static void main(String[] args) {
        SpringApplication.run(ProtectEmailApplication.class, args);
    }

}

@RestController
class ProtectEmailController {

    private static final List<String> PROTECTED_DOMAINS = Arrays.asList("example.com", "sample.net", "gmail.com");
    private static final Pattern EMAIL_PATTERN = Pattern.compile("[^@]+@[^@]+\\.[^@]+");

    @PostMapping("/protect")
    public ResponseEntity<?> protectEmail(@RequestBody EmailRequest request) {
        if (request == null || request.getEmail() == null) {
            return new ResponseEntity<>(new ErrorResponse("メールアドレスが送信されていません"), HttpStatus.BAD_REQUEST);
        }

        String email = request.getEmail().toLowerCase();
        Matcher matcher = EMAIL_PATTERN.matcher(email);

        if (!matcher.matches()) {
            return new ResponseEntity<>(new ErrorResponse("無効なメールアドレス形式です"), HttpStatus.BAD_REQUEST);
        }

        String domain = email.split("@")[1];

        if (PROTECTED_DOMAINS.contains(domain)) {
            String username = email.split("@")[0];
            String maskedUsername = username.substring(0, 2) + "***";
            String maskedEmail = maskedUsername + "@" + domain;
            return ResponseEntity.ok(new ProtectedEmailResponse(maskedEmail));
        } else {
            return ResponseEntity.ok(new MessageResponse("このドメインのメールアドレスは保護対象外です"));
        }
    }
}

class EmailRequest {
    private String email;

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email;
    }
}

class ProtectedEmailResponse {
    private String protected_email;

    public ProtectedEmailResponse(String protected_email) {
        this.protected_email = protected_email;
    }

    public String getProtected_email() {
        return protected_email;
    }
}

class MessageResponse {
    private String message;

    public MessageResponse(String message) {
        this.message = message;
    }

    public String getMessage() {
        return message;
    }
}

class ErrorResponse {
    private String error;

    public ErrorResponse(String error) {
        this.error = error;
    }

    public String getError() {
        return error;
    }
}
