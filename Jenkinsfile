node("linux") {
	timestamps {
		stage ("Checkout") {
			checkout scm
		}

		stage ("Deploy") {
			dir ("src/AeBlog") {
				sh "dotnet lambda deploy-serverless"
			}
		}
	}
}