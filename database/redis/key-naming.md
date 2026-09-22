# Redis Key Naming Policy

Keys must follow exactly:
`smart:{service}:{resource}:{scope}:{identifier}`

## Example
`smart:identity:session:{sessionId}`
`smart:fleet:vehicle:{tenantId}:{vehicleId}`

Arbitrary keys are strictly forbidden. Uncontrolled keys will be dropped by automated pipeline validations.
