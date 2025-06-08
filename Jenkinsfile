pipeline {
    agent any

    environment {
        APP_POOL_NAME = 'BusManagementAPI'
        PROJECT_PATH = "${env.WORKSPACE}\\BusManagementAPI.csproj"
        PUBLISH_DIR = "D:\\C# Projects\\BusManagement\\BusManagementAPI\\bin\\Release\\net7.0\\publish"
    }

    stages {
        stage('Stop IIS App Pool') {
            steps {
                powershell """
                    Import-Module WebAdministration
                    Write-Host "Stopping App Pool: ${APP_POOL_NAME}"
                    Stop-WebAppPool -Name '${APP_POOL_NAME}'
                """
            }
        }

        stage('Build') {
            steps {
                powershell """
                    Write-Host "Building project..."
                    dotnet build '${PROJECT_PATH}' --configuration Release
                """
            }
        }

        stage('Publish') {
            steps {
                powershell """
                    Write-Host "Publishing project to ${PUBLISH_DIR}"
                    dotnet publish '${PROJECT_PATH}' --configuration Release --output '${PUBLISH_DIR}'
                """
            }
        }

        stage('Start IIS App Pool') {
            steps {
                powershell """
                    Write-Host "Starting App Pool: ${APP_POOL_NAME}"
                    Start-WebAppPool -Name '${APP_POOL_NAME}'
                """
            }
        }
    }

    post {
        failure {
            powershell """
                Write-Host "Build failed, attempting to start IIS App Pool to restore state."
                Start-WebAppPool -Name '${APP_POOL_NAME}'
            """
        }
    }
}
