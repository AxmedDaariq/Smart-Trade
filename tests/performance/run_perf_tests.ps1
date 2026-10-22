# PowerShell script to run JMeter performance tests via Docker

$tests = @(
    @{ name = "baseline"; file = "baseline_test.jmx" },
    @{ name = "load"; file = "load_test.jmx" },
    @{ name = "stress"; file = "stress_test.jmx" }
)

foreach ($test in $tests) {
    Write-Host "Running $($test.name) test..." -ForegroundColor Cyan

    # Run JMeter in non-GUI mode using Docker
    docker run --rm `
      -v ${PWD}/performance:/tests `
      -v ${PWD}/performance/results:/results `
      justb solving/jmeter `
      -n -t /tests/$($test.file) `
      -l /results/$($test.name)_results.jtl `
      -e /results/$($test.name)_report

    Write-Host "$($test.name) test completed. Report generated in results/$($test.name)_report" -ForegroundColor Green
}

Write-Host "All performance tests completed successfully!" -ForegroundColor Yellow
