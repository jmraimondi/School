TITLE      (calc.asm)

; This program calculates the area of any polygon
; Last update:
; John Michael Raimondi

INCLUDE Irvine32.inc
.data
CR	EQU	0Dh
LF	EQU	0Ah
ESCAPE	EQU	1Bh
SPC	EQU	20h
multOP	EQU	2Ah
addOP	EQU	2Bh
subOP	EQU	2Dh
divOP	EQU	2Fh

fd	dword	?
ofilename	byte	"output.out", 0

polySides	dword	0,0, 0,4, 4,4, 4,0, 0,0, 100, 0,0, 4,8, 8,0, 0,0, 100
			dword	0,2, 0,12, 9,11, 9,0, 0,2, 100
			dword	1,0, 2,2, 0,9, 4,6, 5,11, 6,6, 11,6, 7,4, 8,0, 5,3, 1,0, 100
			dword	0,5, 6,12, 15,12, 16,8, 24,12, 20,0, 16,6, 14,1, 11,4, 4,3, 8,7, 0,5, 100, 200
			byte	"Polygon 1 Area: " ;16
p1			byte	8	dup(SPC)
			byte	"Polygon 2 Area: "
p2			byte	8	dup(SPC)
			byte	"Polygon 3 Area: "
p3			byte	8	dup(SPC)
			byte	"Polygon 4 Area: "
p4			byte	8	dup(SPC)
			byte	"Polygon 5 Area: "
p5			byte	8	dup(SPC)
			byte	0
.code
;----------------------------------------------------------------
betterItoA	PROC	;eax has #
	mov	ebx,10
	mov	ecx,0
	mov	esi,[esp+4] ;offset to store
next:
	cmp	eax,0
	je	done
	mov	edx,0
	div	ebx
	add	dl,30h
	inc	ecx
	push	dx	;esp - 2
	jmp	next
done:
	cmp	ecx,0
	je	realDone
	pop	dx
	mov	[esi],dl
	inc	esi
	dec	ecx
	jmp	done
realDone:
	ret
betterItoA	ENDP
;----------------------------------------------------------------
myWriteToFile	PROC
nextchar:
	mov	al,[edx]
	cmp	al,0
	je	alldone
	mov	eax,fd
	mov	ecx,1
	mov	ebx,edx	;save edx
	call	WriteToFile
	mov	edx,ebx	;restore edx
	inc	edx
	jmp	nextchar
alldone:
	ret
myWriteToFile	ENDP
;----------------------------------------------------------------
findArea	PROC
	mov	edi,offset	p5
	push	edi
	mov	edi,offset	p4
	push	edi
	mov	edi,offset	p3
	push	edi
	mov	edi,offset	p2
	push	edi
	mov	edi,offset	p1
	push	edi	;ItoA setup
nextSide:
	mov	eax,[esi]
	push	eax	;push Xi
	add	esi,4
	mov	eax,[esi]	;Yi to eax
	add	esi,4
	mov	ebx,[esi]	;Xi+1 to ebx
	cmp ebx,100
	je	allSidesDone	;dont forget pop
	mul	ebx	;Yi * Xi+1
	pop	ebx	;Xi to ebx
	push	eax	;push Yi*Xi+1
	add	esi,4
	mov	eax,[esi]	;Yi+1 to eax
	mul	ebx	;Xi * Yi+1
	pop	ebx	;1st product to ebx
	sub	ebx,eax	;1st - 2nd
	inc	ecx	;sum count++
	sub	esi,4
	push	ebx	;push first answer
	jmp nextSide
allSidesDone:
	add	esp,4	;pop unneeded Xi
	mov	eax,0
sumLoop:
	cmp	ecx,0
	je	polyFinished
	pop	ebx
	add	eax,ebx
	dec	ecx
	jmp	sumLoop
polyFinished:
	mov	edi,esi	;save esi to edi
	call	betterItoA	;store ascii answer
	add	esp,4	;pop mem offset
	mov	esi,edi	;restore esi
	add	esi,4	;next poly
	mov	eax,[esi]
	cmp	eax,200
	je	allDone
	mov	ecx,0
	jmp	nextSide
allDone:
	ret
findArea	ENDP
;----------------------------------------------------------------
main PROC
	mov	esi,	offset	polySides
	mov	ecx,0
	mov	eax,0
	mov	ebx,0
	call	findArea
	mov	edx,offset	ofilename
	call	CreateOutputFile
	mov	fd,eax
	mov	edx,offset	p1
	sub	edx,16	;start of characters
	call	myWriteToFile
	mov	eax,fd
	call	CloseFile
	exit
main ENDP
END main