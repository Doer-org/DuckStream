variable "project_id" {
  description = "project id"
  type        = string
}

variable "default_region" {
  description = "The default region for resources"
  type        = string
  default     = "asia-northeast1"
}

variable "service_name" {
  description = "The name of the Cloud Run service"
  type        = string
}

variable "container_image" {
  description = "The container image to run"
  type        = string
}

variable "credential_path" {
  description = "The path to the service account key file"
  type        = string
}
