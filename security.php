<?php
header('Content-Type: application/json; charset=utf-8');

$protectedDomains = ["example.com", "sample.net", "gmail.com"];

function isValidEmail($email) {
    return filter_var($email, FILTER_VALIDATE_EMAIL);
}

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    $json = file_get_contents('php://input');
    $data = json_decode($json, true);

    if (!isset($data['email'])) {
        http_response_code(400);
        echo json_encode(['error' => 'メールアドレスが送信されていません']);
        exit;
    }

    $email = strtolower($data['email']);

    if (!isValidEmail($email)) {
        http_response_code(400);
        echo json_encode(['error' => '無効なメールアドレス形式です']);
        exit;
    }

    $domain = explode('@', $email)[1];

    if (in_array($domain, $protectedDomains)) {
        $username = explode('@', $email)[0];
        $maskedUsername = substr($username, 0, 2) . "***";
        $maskedEmail = $maskedUsername . '@' . $domain;
        echo json_encode(['protected_email' => $maskedEmail]);
    } else {
        echo json_encode(['message' => 'このドメインのメールアドレスは保護対象外です']);
    }
} else {
    http_response_code(405); // Method Not Allowed
    echo json_encode(['error' => 'POSTリクエストのみ受け付けます']);
}
?>
