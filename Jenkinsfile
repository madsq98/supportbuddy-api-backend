pipeline{
    agent any
	
	triggers {
		pollSCM('H/5 * * * *')
	}
	
    stages{
        stage("Build API") {
            steps{
                sh "dotnet build --configuration Release"
                sh "docker-compose build api"
            }
			post {
				always{
					discordSend description: "Jenkins Pipeline Build", footer: "Footer Text", link: env.BUILD_URL, result: currentBuild.currentResult, title: JOB_NAME, webhookURL: "https://discord.com/api/webhooks/951841557940690966/KsQ0net7a-3dNpUttCtGhzeCKQO3vICrGxiHzn_huasmVlmPB-4bnfJBVcdnwr3_dJGX"
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
            			publishCoverage adapters: [coberturaAdapter(path: "GodFestNu.CoreDomainTest/TestResults/*/coverage.cobertura.xml")] 
        			}
    			}
		}
        stage("Clean containers") {
            steps {
                script {
                    try {
                        sh "docker-compose down"
                    }
                    finally { }
                }
            }
        }
        stage("Deploy") {
            steps {
                sh "docker-compose up -d"
            }
        }
    }
}