node("linux") {
	timestamps {
		stage ("Checkout") {
			checkout scm
		}

		stage ("Restore") {
			sh "dotnet restore"
		}

		stage ("Deploy") {
			dir ("src/AeBlog") {
				sh "dotnet lambda deploy-serverless" +
				   " --disable-interactive True" +
				   " --stack-name AeBlog" +
				   " --s3-bucket ae-temp"
			}
		}
	}
}