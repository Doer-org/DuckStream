resource "google_cloud_run_service" "duckstream_api" {
  name     = var.service_name
  location = var.default_region

  template {
    metadata {
      annotations = {
        "autoscaling.knative.dev/minScale" = "0"
        "autoscaling.knative.dev/maxScale" = "1"
      }
    }
    spec {
      containers {
        image = var.container_image
        env {
          name  = "ENVIRONMENT"
          value = var.app_environment
        }
        env {
          name  = "SERVER_PORT"
          value = var.app_server_port
        }
        env {
          name  = "MOCK_DB"
          value = "false"
        }
        env {
          name  = "MOCK_GCS"
          value = "false"
        }
        env {
          name  = "MOCK_ML"
          value = "false"
        }
        env {
          name  = "DB_NAME"
          value = var.app_db_name
        }
        env {
          name  = "DB_USER"
          value = var.app_db_user
        }
        env {
          name  = "DB_PASSWORD"
          value = var.app_db_password
        }
        env {
          name  = "DB_HOST"
          value = var.app_db_host
        }
        env {
          name  = "DB_PORT"
          value = var.app_db_port
        }
        env {
          name  = "GCS_URL"
          value = var.app_gcs_url
        }
        env {
          name  = "GCS_BUCKET_NAME"
          value = var.app_gcs_bucket
        }
        env {
          name  = "GCS_CREDENTIALS"
          value = var.app_gcs_key
        }
        env {
          name  = "ML_URL"
          value = var.app_ml_url
        }
        env {
          name  = "ML_REQUEST_TIMEOUT_SEC"
          value = var.app_ml_timeout
        }
        env {
          name  = "CLIENT_URL"
          value = var.app_client_url
        }
      }
    }
  }

  traffic {
    percent         = 100
    latest_revision = true
  }
}

data "google_iam_policy" "public_access" {
  binding {
    role = "roles/run.invoker"
    members = [
      "allUsers",
    ]
  }
}

resource "google_cloud_run_service_iam_policy" "public_access" {
  location    = google_cloud_run_service.duckstream_api.location
  project     = google_cloud_run_service.duckstream_api.project
  service     = google_cloud_run_service.duckstream_api.name
  policy_data = data.google_iam_policy.public_access.policy_data
}
