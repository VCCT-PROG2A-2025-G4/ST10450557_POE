using System;
using System.Collections.Generic;

namespace ST10450557_POE
{
    public class ResponseGenerator
    {
        public List<string> Keywords { get; } = new List<string>
        {
            "ransomware", "password", "2fa", "cyber security", "firewall", "vpn",
            "social engineering", "malware", "zero-day attack", "safe online",
            "cybersecurity best practices", "password manager", "public wifi",
            "identity theft", "virus and worm", "data breach", "encryption",
            "ddos attack", "spear phishing", "sql injection", "antivirus",
            "software update", "biometrics", "dark web", "brute force attack",
            "secure browsing", "man-in-the-middle", "patch management",
            "credential stuffing"
        };

        public string Generate(string input)
        {
            if (input.Contains("ransomware"))
                return "Ransomware is a type of malware that encrypts your files and demands a ransom. Avoid clicking suspicious links and always backup your data.";
            else if (input.Contains("strong password") || input.Contains("password"))
                return "A strong password is at least 12 characters long, includes uppercase and lowercase letters, numbers, special characters.";
            else if (input.Contains("two-factor authentication") || input.Contains("2fa"))
                return "Two-factor authentication (2FA) adds an extra security layer by requiring a second verification step, like a phone code.";
            else if (input.Contains("cyber security"))
                return "The state of being protected against the criminal or unauthorized use of electronic data, or the measures taken to achieve this.";
            else if (input.Contains("firewall"))
                return "A firewall is a security system that monitors and controls incoming and outgoing network traffic to prevent unauthorized access.";
            else if (input.Contains("vpn"))
                return "A VPN (Virtual Private Network) encrypts your internet connection, making it secure and private, especially on public Wi-Fi.";
            else if (input.Contains("social engineering"))
                return "Social engineering is the manipulation of people into divulging confidential information. Be skeptical of unexpected requests for personal info.";
            else if (input.Contains("malware"))
                return "Malware is any software designed to harm or exploit devices, networks, or users. Protect yourself with antivirus software and avoid suspicious links.";
            else if (input.Contains("zero-day attack"))
                return "A zero-day attack exploits unknown vulnerabilities before developers can release a fix. Keep software updated to reduce risk.";
            else if (input.Contains("how to stay safe online"))
                return "To stay safe online: Use strong passwords, enable 2FA, update software regularly, avoid public Wi-Fi without a VPN, and beware of phishing scams.";
            else if (input.Contains("cybersecurity best practices"))
                return "Cybersecurity best practices include using strong passwords, enabling firewalls, keeping software updated, and avoiding suspicious downloads.";
            else if (input.Contains("password manager"))
                return "A password manager securely stores and manages your passwords, reducing the risk of weak or reused passwords.";
            else if (input.Contains("public wifi"))
                return "Wi-Fi is risky because hackers can intercept your data. Use a VPN and avoid accessing sensitive accounts on public networks.";
            else if (input.Contains("identity theft"))
                return "Identity theft occurs when someone steals your personal information to commit fraud. Protect yourself by keeping sensitive info private.";
            else if (input.Contains("difference between virus and worm"))
                return "A virus needs a host file to spread, while a worm is self-replicating and spreads without user interaction.";
            else if (input.Contains("data breach"))
                return "A data breach is an unauthorized access of confidential data. Use strong passwords and 2FA to reduce risk.";
            else if (input.Contains("what is encryption"))
                return "Encryption converts information into a secure code to prevent unauthorized access. End-to-end encryption ensures data remains private.";
            else if (input.Contains("ddos attack"))
                return "A DDoS (Distributed Denial-of-Service) attack overwhelms a website or server with excessive traffic from multiple sources, making it inaccessible. Protect against it with rate limiting and cloud-based mitigation services.";
            else if (input.Contains("spear phishing"))
                return "Spear phishing is a targeted phishing attack aimed at specific individuals or organizations, often using personalized information to trick victims. Always verify the sender’s identity before sharing sensitive data.";
            else if (input.Contains("sql injection"))
                return "SQL injection is a cyberattack where hackers insert malicious SQL code into input fields to manipulate a database. Prevent it by using parameterized queries and input validation.";
            else if (input.Contains("antivirus"))
                return "Antivirus software detects and removes malware, such as viruses and ransomware, from your device. Keep it updated and run regular scans to stay protected.";
            else if (input.Contains("software update"))
                return "Software updates fix security vulnerabilities, improve performance, and add features. Install them promptly to protect against exploits like zero-day attacks.";
            else if (input.Contains("biometrics"))
                return "Biometrics use unique physical traits, like fingerprints or facial recognition, for authentication. They’re secure but should be paired with other methods, like passwords, for extra protection.";
            else if (input.Contains("dark web"))
                return "The dark web is a hidden part of the internet accessible only through special software, often used for illegal activities like selling stolen data. Avoid sharing personal information to reduce your risk.";
            else if (input.Contains("brute force attack"))
                return "A brute force attack tries countless password combinations to gain access to an account. Use strong, unique passwords and enable account lockouts to defend against it.";
            else if (input.Contains("secure browsing"))
                return "Secure browsing involves using HTTPS websites, avoiding suspicious links, enabling private browsing modes, and using browser extensions to block trackers and ads.";
            else if (input.Contains("man-in-the-middle"))
                return "A man-in-the-middle (MITM) attack intercepts communication between two parties to steal data or manipulate messages. Use encrypted connections like HTTPS and VPNs to prevent it.";
            else if (input.Contains("patch management"))
                return "Patch management involves regularly applying software updates to fix vulnerabilities. Automating patches and prioritize critical updates to reduce cyber risks.";
            else if (input.Contains("credential stuffing"))
                return "Credential stuffing uses stolen usernames and passwords from one site to access other accounts. Use unique passwords for each account and enable 2FA to protect yourself.";
            else
                return "I didn’t quite understand that. Could you rephrase?";
        }

        public string GenerateMore(string topic)
        {
            switch (topic)
            {
                case "ransomware":
                    return "Ransomware often spreads via phishing emails or exploit kits. Use endpoint protection and train employees to recognize suspicious emails.";
                case "password":
                    return "Avoid reusing passwords across sites. Consider using a passphrase, like 'SunnyHill2023!', for better memorability and security.";
                case "2fa":
                    return "2FA can use SMS codes, authenticator apps, or hardware tokens. Apps like Google Authenticator are more secure than SMS.";
                case "cyber security":
                    return "Cybersecurity frameworks like NIST and ISO 27001 guide organizations in protecting data and systems effectively.";
                case "firewall":
                    return "Modern firewalls include intrusion prevention systems (IPS) and can be hardware or software-based, like Windows Defender Firewall.";
                case "vpn":
                    return "VPNs mask your IP address and are crucial for remote work. Choose providers with no-logs policies for maximum privacy.";
                case "social engineering":
                    return "Common social engineering tactics include pretexting, baiting and tailgating. Regular awareness training can reduce risks.";
                case "malware":
                    return "Malware types include trojans, spyware, adware and spyware. Sandboxing and behavior analysis help detect advanced threats.";
                case "zero-day attack":
                    return "Zero-day exploits are often sold on the dark web. Intrusion detection systems can help identify unusual activity early.";
                case "safe online":
                    return "Use browser security features like anti-tracking extensions and enable DNS over HTTPS for safer browsing.";
                case "cybersecurity best practices":
                    return "Implement least privilege access, conduct regular audits and use SIEM tools for real-time threat monitoring.";
                case "password manager":
                    return "Popular password managers include LastPass, 1Password, and Bitwarden. They generate and autofill complex passwords.";
                case "public wifi":
                    return "Avoid online banking on public Wi-Fi. If necessary, use a personal hotspot or a trusted VPN service.";
                case "identity theft":
                    return "Freeze your credit with agencies like Experian to prevent unauthorized accounts if you suspect identity theft.";
                case "virus and worm":
                    return "Worms can consume network bandwidth, while viruses often corrupt files. Both require robust antivirus solutions.";
                case "data breach":
                    return "Data breaches cost companies millions annually. Encryption and access controls are critical to minimize impact.";
                case "encryption":
                    return "AES-256 is a widely used encryption standard. Quantum computing may challenge current encryption methods in the future.";
                case "ddos attack":
                    return "DDoS attacks can last hours or days. Content Delivery Networks (CDNs) like Cloudflare help absorb malicious traffic.";
                case "spear phishing":
                    return "Spear phishing emails often mimic trusted contacts. Check email headers and avoid clicking links in unsolicited emails.";
                case "sql injection":
                    return "SQL injection can expose sensitive database data. Use ORM frameworks like Entity Framework to reduce vulnerabilities.";
                case "antivirus":
                    return "Modern antivirus solutions use machine learning to detect zero-day threats. Free options like Windows Defender are often sufficient.";
                case "software update":
                    return "Automatic updates reduce human error. Patch management tools like WSUS help organizations stay current.";
                case "biometrics":
                    return "Biometric data, if compromised, can’t be changed. Use multiple-factor authentication to enhance biometric security.";
                case "dark web":
                    return "Dark web marketplaces sell stolen credentials and hacking tools. Monitoring services can alert you if your data appears there.";
                case "brute force attack":
                    return "CAPTCHA and rate-limiting APIs can deter brute force attacks. Strong passwords remain the first line of defense.";
                case "secure browsing":
                    return "Browsers like Firefox offer enhanced privacy settings. Extensions like uBlock Origin block malicious scripts.";
                case "man-in-the-middle":
                    return "MITM attacks exploit unencrypted Wi-Fi or fake SSL certificates. Always verify HTTPS certificate validity.";
                case "patch management":
                    return "Unpatched systems are prime targets. Tools like SCCM automate patch deployment across enterprises.";
                case "credential stuffing":
                    return "Credential stuffing succeeds due to password reuse. Password managers and 2FA significantly reduce this risk.";
                default:
                    return "No additional information available for this topic.";
            }
        }
    }
}