pipeline{
    agent any
	
	triggers {
		pollSCM('H/5 * * * *')
	}
	
    stages{
		stage("Cleanup") {
			steps{
				sh "rm -r -f SB.CoreDomainTest/TestResults"
			}
		}
        stage("Build API") {
			when {
				anyOf {
					changeset "SB.Core/**"
					changeset "SB.CoreDomainTest/**"
					changeset "SB.Domain/**"
					changeset "SB.EFCore/**"
					changeset "SB.WebAPI/**"
				}
			}
            steps{
                sh "dotnet build --configuration Release"
                sh "docker-compose --env-file config/Stage.env build api"
            }
			post {
				always{
					withCredentials([string(credentialsId: 'WEBHOOKURL', variable: 'WEBHOOK_URL')]) {
						discordSend description: "Jenkins Pipeline Build", footer: "Footer Text", link: env.BUILD_URL, result: currentBuild.currentResult, title: JOB_NAME, webhookURL: "${WEBHOOK_URL}"
					}
				}
			}
        }
        stage("Test") {
   			steps {
        			dir("SB.CoreDomainTest") {
            				sh "dotnet add package coverlet.collector"
            				sh "dotnet test --collect:'XPlat Code Coverage'"
        			}
    			}
    		post {
        		success {
            			publishCoverage adapters: [coberturaAdapter(path: "SB.CoreDomainTest/TestResults/*/coverage.cobertura.xml")] 
        			}
    			}
		}
        stage("Clean containers") {
            steps {
                script {
                    try {
                        sh "docker-compose --env-file config/Stage.env down"
                    }
                    finally { }
                }
            }
        }
        stage("Deploy") {
            steps {
                sh "docker-compose --env-file config/Stage.env up -d"
            }
        }
		stage("Push to registry") {
			steps {
				sh "docker-compose --env-file config/Stage.env push"
			}
		}
    }
}
