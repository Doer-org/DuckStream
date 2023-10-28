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
      }
    }
  }

  traffic {
    percent         = 100
    latest_revision = true
  }
}

data "google_iam_policy" "noauth" {
  binding {
    role = "roles/run.invoker"
    members = [
      "allUsers",
    ]
  }
}

resource "google_cloud_run_service_iam_policy" "noauth" {
  location = google_cloud_run_service.duckstream_api.location
  project  = google_cloud_run_service.duckstream_api.project
  service  = google_cloud_run_service.duckstream_api.name

  policy_data = data.google_iam_policy.noauth.policy_data
}
