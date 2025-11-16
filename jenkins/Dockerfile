# FROM jenkins/jenkins:2.528.1-jdk21
# USER root
# RUN apt-get update && apt-get install -y lsb-release
# RUN curl -fsSLo /usr/share/keyrings/docker-archive-keyring.asc \
#   https://download.docker.com/linux/debian/gpg
# RUN echo "deb [arch=$(dpkg --print-architecture) \
#   signed-by=/usr/share/keyrings/docker-archive-keyring.asc] \
#   https://download.docker.com/linux/debian \
#   $(lsb_release -cs) stable" > /etc/apt/sources.list.d/docker.list
# RUN apt-get update && apt-get install -y docker-ce-cli
# USER jenkins
# RUN jenkins-plugin-cli --plugins "blueocean docker-workflow json-path-api"

# FROM jenkins/jenkins:2.528.1-jdk21

# # Switch to root to install system packages
# USER root

# # Basic utilities + Docker CLI + Python 3
# RUN apt-get update && \
#     apt-get install -y lsb-release curl python3 python3-venv python3-pip && \
#     curl -fsSLo /usr/share/keyrings/docker-archive-keyring.asc \
#       https://download.docker.com/linux/debian/gpg && \
#     echo "deb [arch=$(dpkg --print-architecture) \
#       signed-by=/usr/share/keyrings/docker-archive-keyring.asc] \
#       https://download.docker.com/linux/debian \
#       $(lsb_release -cs) stable" > /etc/apt/sources.list.d/docker.list && \
#     apt-get update && \
#     apt-get install -y docker-ce-cli && \
#     # optional: provide `python` alias
#     ln -s /usr/bin/python3 /usr/bin/python || true && \
#     # clean apt cache to keep image smaller
#     rm -rf /var/lib/apt/lists/*

# # Back to jenkins user for runtime
# USER jenkins

# # Install Jenkins plugins
# RUN jenkins-plugin-cli --plugins "docker-workflow json-path-api"

FROM jenkins/jenkins:2.528.1-jdk21

USER root

# Basic tools
RUN apt-get update && \
    apt-get install -y curl wget gnupg lsb-release python3 python3-venv python3-pip nodejs npm unzip && \
    rm -rf /var/lib/apt/lists/*

# Install Google Chrome Stable
RUN curl -fsSL https://dl.google.com/linux/linux_signing_key.pub | gpg --dearmor -o /usr/share/keyrings/google.gpg && \
    echo "deb [arch=amd64 signed-by=/usr/share/keyrings/google.gpg] http://dl.google.com/linux/chrome/deb/ stable main" > /etc/apt/sources.list.d/google-chrome.list && \
    apt-get update && \
    apt-get install -y google-chrome-stable && \
    rm -rf /var/lib/apt/lists/*

# Install matching ChromeDriver automatically
RUN CHROME_VERSION=$(google-chrome --version | awk '{print $3}' | cut -d'.' -f1) && \
    DRIVER_URL=$(curl -s https://chromedriver.storage.googleapis.com/LATEST_RELEASE_$CHROME_VERSION) && \
    wget -O /tmp/chromedriver.zip https://chromedriver.storage.googleapis.com/${DRIVER_URL}/chromedriver_linux64.zip && \
    unzip /tmp/chromedriver.zip -d /usr/local/bin/ && \
    chmod +x /usr/local/bin/chromedriver && \
    rm /tmp/chromedriver.zip

USER jenkins

RUN jenkins-plugin-cli --plugins "docker-workflow json-path-api"
