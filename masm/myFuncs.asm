;----------------------------------------------------------------
myWriteLine PROC	;prints starting at esi stopping at a 0
Next:
	mov	al,[esi]
	cmp	al,0
	je	lineout
	call	WriteChar
	inc	esi
	jmp	Next
lineout:
	ret
myWriteLine	ENDP
;----------------------------------------------------------------
ItoA	PROC	;eax has #
	mov	ebx,10
next:
	cmp	eax,0
	je	done
	mov	edx,0
	div	ebx
	add	dl,30h
	dec	esi
	mov	[esi],dl
	jmp	next
done:
	ret
ItoA	ENDP
;----------------------------------------------------------------
countDigits	PROC
	mov	ebx,10
	mov	ecx,0
next:
	cmp	eax,0
	je	done
	mov	edx,0
	div	ebx
	inc	ecx
	jmp	next
done:
	ret
countDigits	ENDP
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
myReadLine	PROC
nextread:
	call	ReadChar
	mov	[esi],al
	call	WriteChar
	cmp	al,CR
	je	outofhere
	inc	esi
	jmp	nextread
outofhere:
	ret
myReadLine	ENDP
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
myPop	PROC
	cmp	esi,stk + sizeof stk
	je	outofhere
	mov	eax,[esi]
	add	esi,4
outofhere:
	ret
myPop	ENDP
;----------------------------------------------------------------
myPush	PROC
	cmp	esi,stk
	je	full
	add	esi,-4
	mov	[esi],eax
full:
	ret
myPush	ENDP
;----------------------------------------------------------------