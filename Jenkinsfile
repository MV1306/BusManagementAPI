pipeline {
    agent any

    environment {
        APP_POOL_NAME = 'BusManagementAPI'
        PUBLISH_PATH = 'D:\\C# Projects\\BusManagement\\BusManagementAPI\\bin\\Release\\net8.0\\publish'
        PROJECT_PATH = '"C:\\ProgramData\\Jenkins\\.jenkins\\workspace\\Bus Management API\\BusManagementAPI.csproj"'  // <-- Update this path
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Stop IIS App Pool') {
            steps {
                echo "Stopping IIS App Pool: ${env.APP_POOL_NAME}"
                powershell '''
                    Import-Module WebAdministration
                    Stop-WebAppPool -Name "${env.APP_POOL_NAME}"
                    Start-Sleep -Seconds 3
                '''
            }
        }

        stage('Build & Publish') {
            steps {
                echo 'Building and publishing project...'
                bat """
                    dotnet build "${env.PROJECT_PATH}" --configuration Release
                    dotnet publish "${env.PROJECT_PATH}" --configuration Release --output "${env.PUBLISH_PATH}"
                """
            }
        }

        stage('Start IIS App Pool') {
            steps {
                echo "Starting IIS App Pool: ${env.APP_POOL_NAME}"
                powershell '''
                    Import-Module WebAdministration
                    Start-WebAppPool -Name "${env.APP_POOL_NAME}"
                '''
            }
        }
    }

    post {
        success {
            echo '✅ Build and publish successful. App Pool restarted.'
        }
        failure {
            echo '❌ Build or publish failed. Please check logs.'
            // Optionally start the app pool here if you want it running on failure too
        }
    }
}
