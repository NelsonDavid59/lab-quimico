/**
 * https://medium.com/@easylob/a-alternative-way-to-use-visual-studio-with-npm-5427d938ff05#how-does-visual-studio-deal-with-gulp
 */

var gulp = require('gulp');

var source = './node_modules/';
var destination = './wwwroot/npm/';

// Copiar IMASK
gulp.task('copy-imask', function () {
    return gulp.src(source + 'imask/dist/*.{js,map}')
        .pipe(gulp.dest(destination + 'imask/'));
});

// Limpiar carpeta npm (usa import dinámico solo aquí)
gulp.task('clean-npm', async function () {
    const del = (await import('del')).deleteAsync;
    return del([destination + '/**/*']);
});

// Tarea por defecto
gulp.task('default', gulp.series('clean-npm', 'copy-imask'));