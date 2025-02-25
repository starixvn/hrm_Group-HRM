@Library("JenkinsPipelineLibrary") _

pipeline {
  agent any

  options{
    buildDiscarder(logRotator(numToKeepStr: '5'))
    disableConcurrentBuilds()
    gitLabConnection('Gitlab')
  }

  triggers{
    gitlab(triggerOnPush: true, triggerOnMergeRequest: true, branchFilterType: 'All')
  }

  stages {
    stage('Build') {
      steps {
        script{
          pipelineLib.beginSonarQubeForMsBuild("SMSHRM", "Nexus / SMS HRM", "/d:sonar.cs.opencover.reportsPaths=%WORKSPACE%\\coverage.xml")

          pipelineLib.restoreNuget()

          pipelineLib.msBuild("NexHR.sln")
        }
      }
    }

    stage("SonarQube Analysis"){
      steps{
        script{
          pipelineLib.endSonarQubeForMsBuild()
        }
      }

      post{
        always{
          script{
            if(manager.logContains(".*Quality gate status.*")){
              pipelineLib.generateSonarQubeReport("SMSHRM")
            }
          }
        }
      }
    }
  }

  post{
    always{
      archiveArtifacts "*.xml,*.email"
      step([$class: 'ACIPluginPublisher', name: '*.xml', shownOnProjectPage: false])
    }

    unsuccessful{
      emailext body: '$DEFAULT_CONTENT', subject: '$DEFAULT_SUBJECT', to: 'quinn.vo@starixsoft.com,tim.trinh@starixsoft.com'
    }
  }
}