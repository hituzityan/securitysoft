from flask import Flask, request, jsonify
import re

app = Flask(__name__)

PROTECTED_DOMAINS = ["example.com", "sample.net", "gmail.com"]

def is_valid_email(email):
    """簡易的なメールアドレスの形式チェック"""
    return re.match(r"[^@]+@[^@]+\.[^@]+", email)

@app.route('/protect', methods=['POST'])
def protect_email():
    data = request.get_json()
    if not data or 'email' not in data:
        return jsonify({'error': 'メールアドレスが送信されていません'}), 400

    email = data['email'].lower()

    if not is_valid_email(email):
        return jsonify({'error': '無効なメールアドレス形式です'}), 400

    domain = email.split('@')[1]

    if domain in PROTECTED_DOMAINS:
        username = email.split('@')[0]
        masked_username = username[:2] + "***"
        masked_email = f"{masked_username}@{domain}"
        return jsonify({'protected_email': masked_email}), 200
    else:
        return jsonify({'message': 'このドメインのメールアドレスは保護対象外です'}), 200

if __name__ == '__main__':
    app.run(debug=True)
